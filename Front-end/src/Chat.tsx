import React, { useState, useEffect, useRef, KeyboardEvent } from 'react';
import ReactMarkdown from 'react-markdown';

interface Mensagem {
    TipoRemetente: TipoRemetente;
    Texto: string;
}

enum TipoRemetente {
    Usuario = 1,
    IA = 2
}

const Chat: React.FC = () => {
const [mensagens, setMensagens] = useState<Mensagem[]>([
  {
    TipoRemetente: TipoRemetente.IA,
    Texto: "Olá! Eu sou o Chatbot que irá lhe ajudar a construir o seu **Plano de Desenvolviemnto Individual (PDI)** utilizando a Inteligência Artifical (IA)!\n\nVamos começar! Qual é o seu nome?",
  },
]);
    const [input, setInput] = useState('');
    const [carregando, setCarregando] = useState(false);
    const mensagensFimRef = useRef<HTMLDivElement | null>(null);
    const [textoDigitando, setTextoDigitando] = useState("Digitando");

    const enviarMensagem = async () => {
        if (!input.trim()) return;

        const novaMensagem: Mensagem = { TipoRemetente: TipoRemetente.Usuario, Texto: input };
        setMensagens((prev) => [...prev, novaMensagem]);
        setInput('');
        setCarregando(true);

        try {
            const resposta = await fetch('http://localhost:5108/Chat/EnviarMensagem', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ Conteudo: input }),
            });

            const respostaBot: Mensagem = { TipoRemetente: TipoRemetente.IA, Texto: await resposta.text() };
            setMensagens((prev) => [...prev, respostaBot]);
        } catch {
            setMensagens((prev) => [
                ...prev,
                { TipoRemetente: TipoRemetente.IA, Texto: 'Erro na comunicação com a API.' },
            ]);
        } finally {
            setCarregando(false);
        }
    };

    const aoPressionarEnter = (e: KeyboardEvent<HTMLInputElement>) => {
        if (e.key === 'Enter') enviarMensagem();
    };

    useEffect(() => {
        mensagensFimRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, [mensagens]);

    useEffect(() => {
        if (!carregando) {
            setTextoDigitando("Digitando");
            return;
        }

        let i = 0;
        const intervalo = setInterval(() => {
            const pontos = ".".repeat((i % 3) + 1);
            setTextoDigitando(`Digitando${pontos}`);
            i++;
        }, 400);

        return () => clearInterval(intervalo);
    }, [carregando]);

    return (
        <div style={estilos.wrapper}>
            <div style={estilos.titulo}>Chatbot PDI</div>
            <div style={estilos.janela}>
                {mensagens.map((m, i) => (
                    <div
                        key={i}
                        style={{
                            ...estilos.mensagem,
                            backgroundColor: m.TipoRemetente === TipoRemetente.IA ? '#444654' : '#343541',
                            alignSelf: 'stretch',
                        }}
                    >
                        <strong style={{ color: '#fff', fontSize: 13 }}>
                            {m.TipoRemetente === TipoRemetente.IA ? 'IA' : 'Você'}:
                        </strong>
                        <ReactMarkdown
                            children={m.Texto}
                            components={{
                                p: ({ node, ...props }) => <p style={{ color: '#d1d5db', marginTop: 6 }} {...props} />,
                                strong: ({ node, ...props }) => <strong style={{ color: 'white' }} {...props} />,
                            }}
                        />
                    </div>
                ))}

                {carregando && (
                    <div style={{ ...estilos.mensagem, alignSelf: 'flex-start', backgroundColor: '#444654' }}>
                        <strong style={{ color: '#fff' }}>IA:</strong>
                        <div style={{ color: '#d1d5db', marginTop: 6 }}>{textoDigitando}</div>
                    </div>
                )}

                <div ref={mensagensFimRef} />
            </div>

            <div style={estilos.inputContainer}>
                <input
                    value={input}
                    onChange={(e) => setInput(e.target.value)}
                    onKeyDown={aoPressionarEnter}
                    placeholder="Envie uma mensagem"
                    style={estilos.input}
                />
                <button onClick={enviarMensagem} style={estilos.botao}>Enviar</button>
            </div>
        </div>
    );
};

export default Chat;

const estilos: Record<string, React.CSSProperties> = {
    wrapper: {
        height: '100vh',
        display: 'flex',
        flexDirection: 'column',
        backgroundColor: '#343541',
        color: '#fff',
        fontFamily: 'system-ui, sans-serif',
    },
    titulo: {
        textAlign: 'center',
        padding: '20px 0',
        fontSize: 22,
        fontWeight: 'bold',
        borderBottom: '1px solid #4f4f4f',
        backgroundColor: '#202123',
    },
    janela: {
        flex: 1,
        overflowY: 'auto',
        padding: '20px',
        display: 'flex',
        flexDirection: 'column',
        gap: 12,
    },
    mensagem: {
        padding: '16px 20px',
        borderRadius: 8,
        backgroundColor: '#444654',
    },
    inputContainer: {
        display: 'flex',
        gap: 8,
        padding: 16,
        borderTop: '1px solid #4f4f4f',
        backgroundColor: '#40414f',
    },
    input: {
        flex: 1,
        padding: 12,
        borderRadius: 6,
        border: '1px solid #6b7280',
        backgroundColor: '#202123',
        color: '#fff',
        fontSize: 14,
        outline: 'none',
    },
    botao: {
        padding: '0 20px',
        backgroundColor: 'white',
        border: 'none',
        color: 'black',
        fontWeight: 'bold',
        borderRadius: 6,
        cursor: 'pointer',
        transition: 'background-color 0.2s ease',
    },
};